using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            string certPW = "1234";
            string certFN = "Rediscovery";

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
                
            }
            // set static resources before we run to provide the HostBuilder with the updated resources
            var appConfig = (IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration>)host.Services.GetService(typeof(IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration>));
            var resources = (Services.IStaticResources)host.Services.GetService(typeof(Services.IStaticResources));

            resources.ExePath = ExePath;
            if (!string.IsNullOrWhiteSpace(appConfig?.Value?.HostIpAddress))
                resources.HostIpAddress = appConfig?.Value?.HostIpAddress;
            else
                resources.HostIpAddress = HostIpAddress;
            if (appConfig?.Value?.HostPort.HasValue == true)
                resources.HostPort = appConfig.Value.HostPort.Value;
            else
                resources.HostPort = HostPort;
            if (appConfig?.Value?.HostPortHttps.HasValue == true)
                resources.HostPortHttps = appConfig.Value.HostPortHttps.Value;
            else
                resources.HostPortHttps = HostPortHttps;

            if (!string.IsNullOrWhiteSpace(appConfig?.Value?.ServerCertificatePassword))
                certPW = appConfig?.Value?.ServerCertificatePassword;
            if (!string.IsNullOrWhiteSpace(appConfig?.Value?.ServerCertificateFriendlyName))
                certFN = appConfig?.Value?.ServerCertificateFriendlyName;

            resources.X509Certificate2 = GetX509Certificate2(HostIpAddress, certPW, certFN);
            resources.PEM = CertPEM(resources.X509Certificate2);

            Version version = Assembly.GetEntryAssembly().GetName().Version;

            SharedBase.Core.Version appMinimumVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = null };
            if (!string.IsNullOrWhiteSpace(appConfig?.Value?.AppMinimumVersion))
            {
                appMinimumVersion = SharedBase.Core.Version.ConvertTo(appConfig.Value?.AppMinimumVersion);
            }
            SharedBase.Core.Version clientVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = null };
            if (!string.IsNullOrWhiteSpace(appConfig?.Value?.AppMinimumVersion))
            {
                clientVersion = SharedBase.Core.Version.ConvertTo(appConfig.Value?.ServiceVersion);
            }

            resources.ServiceManifest = new SharedBase.Connection.Manifest
            {
                AppMinimumVersion = appMinimumVersion,
                ClientVersion = clientVersion,
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

        private static X509Certificate2 GetX509Certificate2(string host, string password, string friendlyName)
        {
            var pfx = CertificateService.ServerCertificate.CreatePfx(host, password, friendlyName);
            return new X509Certificate2(pfx, password);
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