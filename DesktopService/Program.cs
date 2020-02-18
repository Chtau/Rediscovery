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

namespace DesktopService
{
    internal class Program
    {
        // TODO: https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-2.2
        // TODO: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.2&tabs=visual-studio

        public static string HostIpAddress = "127.0.0.1";

        public static void Main(string[] args)
        {
            HostIpAddress = SharedFeatureFunctions.NetworkAddress.GetIpAddr();

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
                //serverOptions.Listen(System.Net.IPAddress.Parse("192.168.1.100"), 44341);
                serverOptions.Listen(System.Net.IPAddress.Parse(HostIpAddress), 44341);
                serverOptions.ListenLocalhost(44341);
                serverOptions.ListenAnyIP(44341);
                serverOptions.ConfigureEndpointDefaults(listenOptions =>
                {
                    // Configure endpoint defaults
                    
                });
            })
            .UseStartup<Startup>();
        });
    }
}