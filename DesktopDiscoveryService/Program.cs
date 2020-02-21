using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DesktopDiscoveryService
{
    class Program
    {
        private const string CommandAddFirewall = "--addfw";
        private const string CommandRemoveFirewall = "--removefw";
        private const string CommandServiceIPAddr = "--serviceip";
        private const string CommandDiscoveryPort = "--discoveryport";
        private const string CommandServiceMetaInfo = "--servicemeta";

        static void Main(string[] args)
        {
            int discoveryPort = 8888;
            if (args.Any(x => x.StartsWith(CommandDiscoveryPort + ":", StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(CommandDiscoveryPort + ":", StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (int.TryParse(vals[1].Trim(), out int port))
                    discoveryPort = port;
            }
            if (args.Any(x => x.StartsWith("?", StringComparison.OrdinalIgnoreCase) || x.StartsWith("help", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Help for Rediscovery Discovery Service");
                Console.WriteLine("Arguments");
                Console.WriteLine($"    {CommandAddFirewall}    \"Creates Windows Firewall Rule\"");
                Console.WriteLine($"    {CommandRemoveFirewall}    \"Removes Windows Firewall Rule\"");
                Console.WriteLine($"    {CommandServiceIPAddr}    \"Service IP Address for discovery response\"");
                Console.WriteLine($"    {CommandDiscoveryPort}    \"Port to use for the Discovery service\"");
                Console.WriteLine($"    {CommandServiceMetaInfo}    \"Additional Service Metadata for the discovery response\"");
            } else if (args.Any(x => x.StartsWith(CommandAddFirewall, StringComparison.OrdinalIgnoreCase)))
            {
                if (!FirewallRule.DiscoveryRuleExists())
                {
                    if (FirewallRule.DiscoveryRuleCreate())
                    {
                        Console.WriteLine("Firewall rule created");
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Could not create Firewall rule (Restart with Administrator rights)");
                        Console.WriteLine("Alternative you can create the Rule with the Command: " + FirewallRule.GetFWRuleCreate());
                        Console.ResetColor();
                    }
                } else
                {
                    Console.WriteLine("Firewall rule already exists");
                }
            } else if (args.Any(x => x.StartsWith(CommandRemoveFirewall, StringComparison.OrdinalIgnoreCase)))
            {
                if (FirewallRule.DiscoveryRuleExists())
                {
                    if (FirewallRule.DiscoveryRuleDelete())
                    {
                        Console.WriteLine("Firewall rule removed");
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Could not remove Firewall rule (Restart with Administrator rights)");
                        Console.WriteLine("Alternative you can create the Rule with the Command: " + FirewallRule.GetFWRuleDelete());
                        Console.ResetColor();
                    }
                } else
                {
                    Console.WriteLine("Firewall rule already removed");
                }
            } else
            {
                if (!FirewallRule.DiscoveryRuleExists())
                {
                    Console.WriteLine("Firewall rule: Missing");
                    if (!FirewallRule.DiscoveryRuleCreate())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Could not create Firewall rule");
                        Console.ResetColor();
                    } else
                    {
                        Console.WriteLine("Firewall rule created");
                    }
                } else
                {
                    Console.WriteLine("Firewall rule: OK");
                }

                // parse arguments for Service IP Address, Service Meta Information and Port
                string ipAddr = null;
                string serviceMetaInfo = "";
                if (args != null)
                {
                    if (args.Any(x => x.StartsWith(CommandServiceIPAddr + ":", StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(CommandServiceIPAddr + ":", StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        ipAddr = vals[1].Trim();
                    }
                    if (args.Any(x => x.StartsWith(CommandServiceMetaInfo + ":", StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(CommandServiceMetaInfo + ":", StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        serviceMetaInfo = vals[1].Trim();
                    }
                }
                if (string.IsNullOrWhiteSpace(ipAddr))
                    ipAddr = SharedFeatureFunctions.NetworkAddress.GetIpAddr();

                Console.WriteLine("Waiting for Clients");
                var dis = new DiscoveryClient();
                dis.Start(ipAddr, serviceMetaInfo, discoveryPort, (client) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Client discover message received from IP:{client}");
                    Console.ResetColor();
                });
                Console.ReadKey();
            }
        }
    }
}
