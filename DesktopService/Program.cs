using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace DesktopService
{
    internal class Program
    {
        // TODO: https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-2.2
        // TODO: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.2&tabs=visual-studio

        public static string HostIpAddress = "127.0.0.1";
        public static ushort HostPort = 44341;
        public static ushort HostPortHttps = 44342;
        public static string ExePath = null;

        public static void Main(string[] args)
        {
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

            //System.Threading.Thread.Sleep(30000);
            //CreateHostBuilder(args).Build().Run();
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
                host.Run();
            }
            else
            {
                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ConfigureHttpsDefaults(op =>
                {
                    //op.AllowAnyClientCertificate();
                    op.ServerCertificate = GetX509Certificate2();
                });

                //serverOptions.Listen(System.Net.IPAddress.Parse("192.168.1.100"), 44341);
                serverOptions.Listen(System.Net.IPAddress.Parse(HostIpAddress), HostPort);
                serverOptions.Listen(System.Net.IPAddress.Parse(HostIpAddress), HostPortHttps, (lo) =>
                {
                    lo.UseHttps();
                });
                serverOptions.ListenLocalhost(HostPort);
                serverOptions.ListenLocalhost(HostPortHttps, (lo) =>
                {
                    lo.UseHttps();
                });
                serverOptions.ListenAnyIP(HostPort);
                serverOptions.ListenAnyIP(HostPortHttps, (lo) =>
                {
                    lo.UseHttps();
                });
                serverOptions.ConfigureEndpointDefaults(listenOptions =>
                {
                    // Configure endpoint defaults
                    
                });
            })
            .UseStartup<Startup>();
        });

        private static X509Certificate2 GetX509Certificate2()
        {
            var cert = new X509Certificate2(Path.Combine(@"C:\DEV\Code\Workspaces\Repos\Rediscovery\TestSignalR", "dev_localhost.pfx"), "1234");
            Console.WriteLine(cert.FriendlyName + " Issuer:" + cert.Issuer + " Thumbprint:" + cert.Thumbprint);
            return cert;
        }
    }
}