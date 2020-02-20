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
            if (args.Any(x => x.StartsWith("-addfw", StringComparison.OrdinalIgnoreCase)))
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
            } else if (args.Any(x => x.StartsWith("-removefw", StringComparison.OrdinalIgnoreCase)))
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

                // TODO: parse arguments for Service IP Address, Service Meta Information and Port

                Console.WriteLine("Waiting for Clients");
                var dis = new DiscoveryClient();
                dis.Start(SharedFeatureFunctions.NetworkAddress.GetIpAddr(), "", 8888, (client) =>
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
