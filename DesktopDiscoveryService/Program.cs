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
        static void Main(string[] args)
        {
            if (args.Any(x => x.StartsWith("--addfw", StringComparison.OrdinalIgnoreCase)))
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
            } else if (args.Any(x => x.StartsWith("--removefw", StringComparison.OrdinalIgnoreCase)))
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
                int discoveryPort = 8888;
                string serviceMetaInfo = "";
                if (args != null)
                {
                    string serviceIPArg = "--serviceip:";
                    if (args.Any(x => x.StartsWith(serviceIPArg, StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(serviceIPArg, StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        ipAddr = vals[1].Trim();
                    }
                    string discoveryPortArg = "--discoveryport:";
                    if (args.Any(x => x.StartsWith(discoveryPortArg, StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(discoveryPortArg, StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        if (int.TryParse(vals[1].Trim(), out int port))
                            discoveryPort = port;
                    }
                    string serviceMetaArg = "--servicemeta:";
                    if (args.Any(x => x.StartsWith(serviceMetaArg, StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(serviceMetaArg, StringComparison.OrdinalIgnoreCase));
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
