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
        private const string DiscoveryServiceRuleName = "Rediscovery discovery Service";

        private const string CommandServiceIPAddr = "--serviceip";
        private const string CommandDiscoveryPort = "--discoveryport";
        private const string CommandServiceMetaInfo = "--servicemeta";
        private const string CommandServicePort = "--serviceport";
        private const string CommandServiceName = "--servicename";

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var sectionServiceInfo = configuration.GetSection("ServiceInfo");
            var sectionDiscovery = configuration.GetSection("Discovery");


            ushort discoveryPort = 8888;
            if (args.Any(x => x.StartsWith(CommandDiscoveryPort + ":", StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(CommandDiscoveryPort + ":", StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (ushort.TryParse(vals[1].Trim(), out ushort port))
                    discoveryPort = port;
            }
            if (args.Any(x => x.StartsWith("?", StringComparison.OrdinalIgnoreCase) || x.StartsWith("help", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Help for Rediscovery Discovery Service");
                Console.WriteLine("Arguments");
                Console.WriteLine($"    {CommandDiscoveryPort}    \"Port to use for the Discovery service\"");
                Console.WriteLine($"    {CommandServiceIPAddr}    \"Service IP Address for discovery response\"");
                Console.WriteLine($"    {CommandServicePort}    \"Service Port for the discovery response\"");
                Console.WriteLine($"    {CommandServiceName}    \"Service Name for the discovery response\"");
                Console.WriteLine($"    {CommandServiceMetaInfo}    \"Additional Service Metadata for the discovery response\"");
            } else
            {
                if (!FirewallRule.DiscoveryRuleExists(discoveryPort, DiscoveryServiceRuleName))
                {
                    Console.WriteLine("Firewall rule: Missing");
                    if (!FirewallRule.DiscoveryRuleCreate(discoveryPort, DiscoveryServiceRuleName, FirewallRule.ProtocolType.UDP))
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
                var serviceInfo = new SharedCoreModels.DiscoveryServiceInfo();
                if (args != null)
                {
                    if (args.Any(x => x.StartsWith(CommandServiceIPAddr + ":", StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(CommandServiceIPAddr + ":", StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        serviceInfo.IPAddress = vals[1].Trim();
                    }
                    if (args.Any(x => x.StartsWith(CommandServiceMetaInfo + ":", StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(CommandServiceMetaInfo + ":", StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        serviceInfo.Metadata = vals[1].Trim();
                    }
                    if (args.Any(x => x.StartsWith(CommandServiceName + ":", StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(CommandServiceName + ":", StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        serviceInfo.Name = vals[1].Trim();
                    }
                    if (args.Any(x => x.StartsWith(CommandServicePort + ":", StringComparison.OrdinalIgnoreCase)))
                    {
                        var valueArg = args.First(x => x.StartsWith(CommandServicePort + ":", StringComparison.OrdinalIgnoreCase));
                        var vals = valueArg.Split(':');
                        if (ushort.TryParse(vals[1].Trim(), out ushort port))
                            serviceInfo.Port = port;
                    }
                }

                Console.WriteLine("Waiting for Clients");
                var dis = new DiscoveryClient();
                dis.Start(serviceInfo, discoveryPort, (client) =>
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
