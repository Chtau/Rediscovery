using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SharedFeatureFunctions;
using Microsoft.Extensions.Configuration;
using System.IO;
using SharedBase.Discovery;

namespace DesktopDiscoveryService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(SharedConfigurations.DiscoveryService.ConfigFileNames.AppSettings, optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var serviceInfoSettings = configuration.GetSection(SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration.SectionName).Get<SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration>();
            var discoverySettings = configuration.GetSection(SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration.SectionName).Get<SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration>();

            if (FirewallRule.RuleExists(discoverySettings.FirewallRuleName, discoverySettings.Port) != FirewallRule.RuleState.True)
            {
                Console.WriteLine("Firewall rule: Missing");
                if (FirewallRule.RuleCreate(discoverySettings.Port, discoverySettings.FirewallRuleName, FirewallRule.ProtocolType.UDP) != FirewallRule.RuleState.True)
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

            var serviceInfo = new DiscoveryServiceInfo
            {
                Port = serviceInfoSettings.Port,
                IPAddress = serviceInfoSettings.IP,
                Metadata = serviceInfoSettings.MetaInfo,
                DesktopName = !string.IsNullOrWhiteSpace(serviceInfoSettings.Name) ? serviceInfoSettings.Name : Environment.MachineName,
                DesktopOS = System.Runtime.InteropServices.RuntimeInformation.OSDescription
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
