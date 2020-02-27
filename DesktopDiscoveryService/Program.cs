using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SharedFeatureFunctions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DesktopDiscoveryService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var serviceInfoSettings = configuration.GetSection("ServiceInfo").Get<SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration>();
            var discoverySettings = configuration.GetSection("Discovery").Get< SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration> ();

            if (!FirewallRule.DiscoveryRuleExists(discoverySettings.Port, discoverySettings.FirewallRuleName))
            {
                Console.WriteLine("Firewall rule: Missing");
                if (!FirewallRule.DiscoveryRuleCreate(discoverySettings.Port, discoverySettings.FirewallRuleName, FirewallRule.ProtocolType.UDP))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not create Firewall rule");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Firewall rule created");
                }
            }
            else
            {
                Console.WriteLine("Firewall rule: OK");
            }

            var serviceInfo = new SharedCoreModels.DiscoveryServiceInfo
            {
                Port = serviceInfoSettings.Port,
                IPAddress = serviceInfoSettings.IP,
                Metadata = serviceInfoSettings.MetaInfo,
                Name = serviceInfoSettings.Name
            };

            Console.WriteLine("Waiting for Clients");
            var dis = new DiscoveryClient();
            dis.Start(serviceInfo, discoverySettings.Port, (client) =>
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Client discover message received from IP:{client}");
                Console.ResetColor();
            });
            Console.ReadKey();
        }
    }
}
