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

                Console.WriteLine("Waiting for Clients");
                var dis = new DiscoveryClient();
                dis.Start((client) =>
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
