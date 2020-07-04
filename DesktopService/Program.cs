using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DesktopService
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string DesktopName = "Desktop";
            string HostIpAddress = "127.0.0.1";
            ushort HostPort = 44341;
            ushort HostPortHttps = 44342;
            string ExePath = null;

            HostIpAddress = SharedFeatureFunctions.NetworkAddress.GetIpAddr();
            ExePath = Process.GetCurrentProcess().MainModule.FileName;

            if (args.Any(x => x.StartsWith(SharedCommandArguments.Service.Arguments.CommandPort, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Service.Arguments.CommandPort, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (ushort.TryParse(vals[1].Trim(), out ushort port))
                    HostPort = port;
            }
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Service.Arguments.CommandPortHttps, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Service.Arguments.CommandPortHttps, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (ushort.TryParse(vals[1].Trim(), out ushort port))
                    HostPortHttps = port;
            }
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Service.Arguments.CommandIP, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Service.Arguments.CommandIP, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                HostIpAddress = vals[1].Trim();
            }
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Service.Arguments.DesktopName, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Service.Arguments.DesktopName, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                DesktopName = vals[1].Trim();
            }

            var isService = false;// !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            var builder = CreateHostBuilder(
                args.Where(arg => arg != "--console").ToArray());

            var host = builder.Build();

            if (isService)
            {
                // To run the app without the CustomWebHostService change the
                // next line to host.RunAsService();
                //host.RunAsCustomService();
                //host.RunAsService();
            }
            // set static resources before we run to provide the HostBuilder with the updated resources
            var resources = (Services.IStaticResources)host.Services.GetService(typeof(Services.IStaticResources));
            resources.ExePath = ExePath;
            resources.HostIpAddress = HostIpAddress;
            resources.HostPort = HostPort;
            resources.HostPortHttps = HostPortHttps;
            resources.X509Certificate2 = GetX509Certificate2(HostIpAddress);
            resources.PEM = CertPEM(resources.X509Certificate2);

            Version version = Assembly.GetEntryAssembly().GetName().Version;
            resources.ServiceManifest = new SharedBase.Connection.Manifest
            {
                AppMinimumVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                ClientVersion = new SharedBase.Core.Version() { Major = version.Major, Minor = version.Minor, Patch = version.Revision, Label = null },
                SupportedFeatures = new System.Collections.Generic.List<SharedBase.Device.FeatureDefinitionExtended>(),
                ClientName = DesktopName
            };

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(serverOptions =>
            {
                var res = serverOptions.ApplicationServices.GetService<Services.IStaticResources>();
                serverOptions.ConfigureHttpsDefaults(op =>
                {
                    op.ServerCertificate = res.X509Certificate2;
                });

                serverOptions.Listen(System.Net.IPAddress.Parse(res.HostIpAddress), res.HostPort, so =>
                {
                    so.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
                serverOptions.Listen(System.Net.IPAddress.Parse(res.HostIpAddress), res.HostPortHttps, (lo) =>
                {
                    lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                    lo.UseHttps();
                });
                serverOptions.ConfigureEndpointDefaults(listenOptions =>
                {
                    // Configure endpoint defaults
                    
                });
            })
            .UseStartup<Startup>();
        });

        private static X509Certificate2 GetX509Certificate2(string host)
        {
            var pfx = CertificateService.ServerCertificate.CreatePfx(host, "1234", "Rediscovery");
            return new X509Certificate2(pfx, "1234");
        }

        private static string CertPEM(X509Certificate2 cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }
    }
}