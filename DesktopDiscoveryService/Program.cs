using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Rediscovery.Feature.Shared.Functions;
using Microsoft.Extensions.Configuration;
using System.IO;
using Rediscovery.Shared.Base.Discovery;

namespace Rediscovery.Client.Service.Discovery
{
    static class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(Rediscovery.Shared.Configurations.Discovery.ConfigFileNames.AppSettings, optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var serviceInfoSettings = configuration.GetSection(Rediscovery.Shared.Configurations.Discovery.Models.ServiceInfoConfiguration.SectionName).Get<Rediscovery.Shared.Configurations.Discovery.Models.ServiceInfoConfiguration>();
            var discoverySettings = configuration.GetSection(Rediscovery.Shared.Configurations.Discovery.Models.DiscoveryConfiguration.SectionName).Get<Rediscovery.Shared.Configurations.Discovery.Models.DiscoveryConfiguration>();

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
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Firewall rule created");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Firewall rule: OK");
                Console.ResetColor();
            }

            var serviceInfo = new DiscoveryServiceInfo
            {
                Port = serviceInfoSettings.Port,
                IPAddress = serviceInfoSettings.IP,
                Metadata = serviceInfoSettings.MetaInfo,
                DesktopName = !string.IsNullOrWhiteSpace(serviceInfoSettings.Name) ? serviceInfoSettings.Name : Environment.MachineName,
                DesktopOS = System.Runtime.InteropServices.RuntimeInformation.OSDescription
            };

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Provided Service Info: \r\n{Newtonsoft.Json.JsonConvert.SerializeObject(serviceInfo, Newtonsoft.Json.Formatting.Indented)}\r\n");
            Console.WriteLine($"Discovery Setting: \r\n{Newtonsoft.Json.JsonConvert.SerializeObject(discoverySettings, Newtonsoft.Json.Formatting.Indented)}\r\n");
            Console.ResetColor();

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
