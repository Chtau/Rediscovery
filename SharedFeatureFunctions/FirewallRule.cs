using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFirewallHelper;

namespace SharedFeatureFunctions
{
    public static class FirewallRule
    {
        public static bool DiscoveryRuleExists(int port, string ruleName)
        {
            var rule = FirewallManager.Instance.Rules.FirstOrDefault(x => x.Name == ruleName);
            if (rule.IsEnable && rule.LocalPorts.Any(x => x == port))
                return true;
            return false;
        }

        public static bool DiscoveryRuleCreate(int port, string ruleName)
        {
            try
            {
                var rule = FirewallManager.Instance.CreatePortRule(FirewallProfiles.Private | FirewallProfiles.Domain,
    ruleName,
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

        public static bool DiscoveryRuleDelete(string ruleName)
        {
            try
            {
                var myRule = FirewallManager.Instance.Rules.SingleOrDefault(r => r.Name == ruleName);
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

        public static string GetFWRuleDelete(int port, string ruleName)
        {
            return $"netsh advfirewall firewall delete rule name=\"{ruleName}\" protocol=UDP localport={port}";
        }

        public static string GetFWRuleCreate(int port, string ruleName)
        {
            return $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow protocol=UDP localport={port}";
        }
    }
}
