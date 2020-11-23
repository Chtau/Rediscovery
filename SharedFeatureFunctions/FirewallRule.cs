using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFirewallHelper;

namespace Rediscovery.Feature.Shared.Functions
{
    public static class FirewallRule
    {
        public enum RuleState
        {
            False,
            True,
            AdminRequired
        }

        public enum ProtocolType
        {
            Any,
            // Virtual Router Redundancy Protocol
            VRRP,
            // Transmission Control Protocol
            TCP,
            // Pragmatic General Multicast Protocol
            PGM,
            // Layer 2 Tunneling Protocol
            L2TP,
            // Internet Protocol Version 6 Route Header
            IPv6Route,
            // Internet Protocol Version 6 Options Header
            IPv6Opts,
            // Internet Protocol Version 6 No Next Header
            IPv6NoNxt,
            // User Datagram Protocol
            UDP,
            // Internet Protocol Version 6
            IPv6,
            // Internet Group Management Protocol
            IGMP,
            // Internet Control Message Protocol for IPv6
            ICMPv6,
            // Internet Control Message Protocol for IPv4
            ICMPv4,
            // Hop-by-Hop Option Protocol
            HOPOPT,
            // Internet Protocol Version 6 Fragmentation Header
            IPv6Frag,
            // Generic Routing Encapsulation Protocol
            GRE
        }

        public static RuleState RuleExists(string ruleName, int port = -1)
        {
            var rule = FirewallManager.Instance.Rules.FirstOrDefault(x => x.Name == ruleName);
            if (rule != null && rule.IsEnable && (port == -1 || rule.LocalPorts.Any(x => x == port)))
                return RuleState.True;
            return RuleState.False;
        }

        public static RuleState RuleCreate(string ruleName, string execFile)
        {
            try
            {
                var rule = FirewallManager.Instance.CreateApplicationRule(FirewallProfiles.Public, ruleName, execFile);
                FirewallManager.Instance.Rules.Add(rule);
                return RuleState.True;
            }
            catch (System.UnauthorizedAccessException)
            {
                // required admin
                return RuleState.AdminRequired;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
                return RuleState.False;
            }
        }

        public static RuleState RuleCreate(ushort port, string ruleName, ProtocolType protocolType)
        {
            try
            {
                var rule = FirewallManager.Instance.CreatePortRule(FirewallProfiles.Private | FirewallProfiles.Domain,
    ruleName,
    FirewallAction.Allow,
    port,
    GetFirewallProtocol(protocolType)
);
                FirewallManager.Instance.Rules.Add(rule);
                return RuleState.True;
            }
            catch (System.UnauthorizedAccessException)
            {
                // required admin
                return RuleState.AdminRequired;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
                return RuleState.False;
            }
        }

        public static RuleState RuleDelete(string ruleName)
        {
            try
            {
                var myRule = FirewallManager.Instance.Rules.SingleOrDefault(r => r.Name == ruleName);
                if (myRule != null)
                {
                    FirewallManager.Instance.Rules.Remove(myRule);
                }
                return RuleState.True;
            }
            catch (System.UnauthorizedAccessException)
            {
                // required admin
                return RuleState.AdminRequired;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString() + Environment.NewLine);
                return RuleState.False;
            }
        }

        public static string GetFWRuleDelete(int port, string ruleName, ProtocolType protocolType)
        {
            return $"netsh advfirewall firewall delete rule name=\"{ruleName}\" protocol={Enum.GetName(typeof(ProtocolType), protocolType)} localport={port}";
        }

        public static string GetFWRuleCreate(int port, string ruleName, ProtocolType protocolType)
        {
            return $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow protocol={Enum.GetName(typeof(ProtocolType), protocolType)} localport={port}";
        }

        private static FirewallProtocol GetFirewallProtocol(ProtocolType protocolType)
        {
            switch (protocolType)
            {
                case ProtocolType.Any:
                    return FirewallProtocol.Any;
                case ProtocolType.VRRP:
                    return FirewallProtocol.VRRP;
                case ProtocolType.TCP:
                    return FirewallProtocol.TCP;
                case ProtocolType.PGM:
                    return FirewallProtocol.PGM;
                case ProtocolType.L2TP:
                    return FirewallProtocol.L2TP;
                case ProtocolType.IPv6Route:
                    return FirewallProtocol.IPv6Route;
                case ProtocolType.IPv6Opts:
                    return FirewallProtocol.IPv6Opts;
                case ProtocolType.IPv6NoNxt:
                    return FirewallProtocol.IPv6NoNxt;
                case ProtocolType.UDP:
                    return FirewallProtocol.UDP;
                case ProtocolType.IPv6:
                    return FirewallProtocol.IPv6;
                case ProtocolType.IGMP:
                    return FirewallProtocol.IGMP;
                case ProtocolType.ICMPv6:
                    return FirewallProtocol.ICMPv6;
                case ProtocolType.ICMPv4:
                    return FirewallProtocol.ICMPv4;
                case ProtocolType.HOPOPT:
                    return FirewallProtocol.HOPOPT;
                case ProtocolType.IPv6Frag:
                    return FirewallProtocol.IPv6Frag;
                case ProtocolType.GRE:
                    return FirewallProtocol.GRE;
            }
            return FirewallProtocol.Any;
        }
    }
}
