using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DesktopService
{
    internal class Program
    {
        // TODO: https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-2.2
        // TODO: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.2&tabs=visual-studio

        public static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            var builder = CreateWebHostBuilder(
                args.Where(arg => arg != "--console").ToArray());

            var host = builder.Build();

            if (isService)
            {
                // To run the app without the CustomWebHostService change the
                // next line to host.RunAsService();
                //host.RunAsCustomService();
                host.RunAsService();
            }
            else
            {
                host.Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Configure the app here.
                })
            .UseKestrel(options => {
                options.ListenAnyIP(44341);
            })
                .UseStartup<Startup>();
    }
}